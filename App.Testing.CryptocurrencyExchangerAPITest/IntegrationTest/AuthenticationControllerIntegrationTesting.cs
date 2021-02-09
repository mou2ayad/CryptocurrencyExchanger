using FluentAssertions;

using System.Threading.Tasks;
using Xunit;

namespace App.Testing.CryptocurrencyExchangerAPITest.IntegrationTest
{
    public class AuthenticationControllerIntegrationTesting
    {
        [Fact]
        public async Task Testing_Authentication_With_Valid_UserAndPasswordAsync()
        {
            //Arrange
            var httpClient=AppClientFactory.GetClient();

            //Act
            await httpClient.AuthenticationAsync("knab", "knab2021");

            //Assert
            httpClient.DefaultRequestHeaders.Authorization.Scheme.Should().StartWith("bearer");
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Testing_Authentication_With_Invalid_UserAsync()
        {
            //Arrange
            var httpClient = AppClientFactory.GetClient();

            //Act
            // invalid userName
            await httpClient.AuthenticationAsync("knab123", "knab2021");

            //Assert
            httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();
        }
        [Fact]
        public async Task Testing_Authentication_With_Invalid_PasswordAsync()
        {
            //Arrange
            var httpClient = AppClientFactory.GetClient();

            //Act
            // invalid Password and Valid userName
            await httpClient.AuthenticationAsync("knab", "knab123");

            //Assert
            httpClient.DefaultRequestHeaders.Authorization.Should().BeNull();
        }
    }
}
