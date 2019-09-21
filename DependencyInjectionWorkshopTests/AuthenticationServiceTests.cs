using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private const string DefaultAccountId = "mystic";
        private const string DefaultPasswordFromDb = "hashed_mypassword";
        private const string DefaultPassword = "mypassword";
        private const string DefaultOtp = "123456";
        private IProfile _profile;
        private IHash _hash;
        private IOtpService _otpService;
        private IFailedCounter _failedCounter;
        private INotification _notification;
        private ILogger _logger;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void SetUp()
        {
            _profile = Substitute.For<IProfile>();
            _hash = Substitute.For<IHash>();
            _otpService = Substitute.For<IOtpService>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _notification = Substitute.For<INotification>();
            _logger = Substitute.For<ILogger>();
            _authenticationService = new AuthenticationService(_profile, _hash, _otpService, _failedCounter, _notification, _logger);
        }

        [Test]
        public void is_valid()
        {
            _profile.GetPassword(DefaultAccountId).Returns(DefaultPasswordFromDb);
            _hash.Compute(DefaultPassword).Returns(DefaultPasswordFromDb);
            _otpService.GetCurrentOtp(DefaultAccountId).Returns(DefaultOtp);

            var valid = IsValid(DefaultAccountId, DefaultPassword, DefaultOtp);
            ShouldBeValid(valid);
        }

        [Test]
        public void is_invalid()
        {
            _profile.GetPassword(DefaultAccountId).Returns(DefaultPasswordFromDb);
            _hash.Compute(DefaultPassword).Returns("wrong password");
            _otpService.GetCurrentOtp(DefaultAccountId).Returns(DefaultOtp);

            var valid = IsValid(DefaultAccountId, DefaultPassword, DefaultOtp);
            ShouldBeInvalid(valid);
        }

        [Test]
        public void reset_failed_count_when_valid()
        {
            WhenValid();
            _failedCounter.Received(1).ResetFailedCount(DefaultAccountId);
        }

        private static void ShouldBeInvalid(bool valid)
        {
            Assert.IsFalse(valid);
        }

        private static void ShouldBeValid(bool valid)
        {
            Assert.IsTrue(valid);
        }

        private bool IsValid(string accountId, string password, string otp)
        {
            return _authenticationService.Verify(accountId, password, otp);
        }

        private void WhenValid()
        {
            _profile.GetPassword(DefaultAccountId).Returns(DefaultPasswordFromDb);
            _hash.Compute(DefaultPassword).Returns(DefaultPasswordFromDb);
            _otpService.GetCurrentOtp(DefaultAccountId).Returns(DefaultOtp);

            IsValid(DefaultAccountId, DefaultPassword, DefaultOtp);
        }
    }
}