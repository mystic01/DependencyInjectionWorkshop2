using DependencyInjectionWorkshop.Repositories;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly IProfile _profileDao;
        private readonly IHash _sha256Adapter;
        private readonly IOtpService _otpService;
        private readonly IFailedCounter _failedCounter;
        private readonly INotification _slackAdapter;
        private readonly ILogger _logger;

        public AuthenticationService(IProfile profileDao, IHash sha256Adapter, IOtpService otpService, IFailedCounter failedCounter, INotification slackAdapter, ILogger logger)
        {
            _profileDao = profileDao;
            _sha256Adapter = sha256Adapter;
            _otpService = otpService;
            _failedCounter = failedCounter;
            _slackAdapter = slackAdapter;
            _logger = logger;
        }

        public AuthenticationService()
        {
            _profileDao = new ProfileDao();
            _sha256Adapter = new Sha256Adapter();
            _otpService = new OtpService();
            _failedCounter = new FailedCounter();
            _slackAdapter = new SlackAdapter();
            _logger = new Logger();
        }

        public bool Verify(string accountId, string password, string otp)
        {
            if (_failedCounter.GetAccountIsLocked(accountId))
            {
                throw new FailedTooManyTimesException();
            }

            var hashedPasswordFromDb = _profileDao.GetPassword(accountId);

            var hashedPassword = _sha256Adapter.Compute(password);

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

                _slackAdapter.Notify(accountId);
                return false;
            }
        }
    }
}