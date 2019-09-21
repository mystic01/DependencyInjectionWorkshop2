using DependencyInjectionWorkshop.Repositories;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtpService _otpService;
        private readonly IFailedCounter _failedCounter;
        private readonly INotification _notification;
        private readonly ILogger _logger;

        public AuthenticationService(IProfile profile, IHash hash, IOtpService otpService, IFailedCounter failedCounter, INotification notification, ILogger logger)
        {
            _profile = profile;
            _hash = hash;
            _otpService = otpService;
            _failedCounter = failedCounter;
            _notification = notification;
            _logger = logger;
        }

        public AuthenticationService()
        {
            _profile = new ProfileDao();
            _hash = new Sha256Adapter();
            _otpService = new OtpService();
            _failedCounter = new FailedCounter();
            _notification = new SlackAdapter();
            _logger = new Logger();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            if (_failedCounter.GetAccountIsLocked(accountId))
            {
                throw new FailedTooManyTimesException();
            }

            var hashedPasswordFromDb = _profile.GetPassword(accountId);

            var hashedPassword = _hash.Compute(password);

            var currentOtp = _otpService.GetCurrentOtp(accountId);

            if (hashedPassword == hashedPasswordFromDb && otp == currentOtp)
            {
                _failedCounter.ResetFailedCount(accountId);
                return true;
            }
            else
            {
                _failedCounter.AddFailedCount(accountId);

                var failedCount = _failedCounter.GetFailedCount(accountId);
                _logger.LogMessage($"accountId:{accountId} failed times:{failedCount}");

                _notification.Notify(accountId);
                return false;
            }
        }
    }
}