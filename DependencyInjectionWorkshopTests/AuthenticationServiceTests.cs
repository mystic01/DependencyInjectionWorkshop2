using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        [Test]
        public void is_valid()
        {
            var profile = Substitute.For<IProfile>();
            var hash = Substitute.For<IHash>();
            var otpService = Substitute.For<IOtpService>();
            var failedCounter = Substitute.For<IFailedCounter>();
            var notification = Substitute.For<INotification>();
            var logger = Substitute.For<ILogger>();

            profile.GetPassword("mystic").Returns("hashed_mypassword");
            hash.Compute("mypassword").Returns("hashed_mypassword");
            otpService.GetCurrentOtp("mystic").Returns("123456");

            var authenticationService = new AuthenticationService(profile, hash, otpService, failedCounter, notification, logger);

            var valid = authenticationService.Verify("mystic", "mypassword", "123456");

            Assert.IsTrue(valid);
        }
    }
}