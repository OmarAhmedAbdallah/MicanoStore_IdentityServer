using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Models;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.Tests
{
    public class UserManagementTests
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementTests()
        {
            var services = new ServiceCollection();
            
            // Setup in-memory database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid()));

            // Setup Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var serviceProvider = services.BuildServiceProvider();
            _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        }

        [Fact]
        public async Task CreateUser_ShouldSucceed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com"
            };

            // Act
            var result = await _userManager.CreateAsync(user, "Test@123");

            // Assert
            Assert.True(result.Succeeded);
            var createdUser = await _userManager.FindByEmailAsync("testuser@example.com");
            Assert.NotNull(createdUser);
            Assert.Equal("testuser@example.com", createdUser.Email);
        }

        [Fact]
        public async Task CreateUserWithInvalidPassword_ShouldFail()
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com"
            };

            // Act
            var result = await _userManager.CreateAsync(user, "weak");

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task AssignRoleToUser_ShouldSucceed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com"
            };
            await _userManager.CreateAsync(user, "Test@123");

            var roleName = "TestRole";
            await _roleManager.CreateAsync(new IdentityRole(roleName));

            // Act
            var result = await _userManager.AddToRoleAsync(user, roleName);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(await _userManager.IsInRoleAsync(user, roleName));
        }

        [Fact]
        public async Task UserAuthentication_ShouldSucceed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                UserName = "testuser@example.com",
                Email = "testuser@example.com"
            };
            await _userManager.CreateAsync(user, "Test@123");

            // Act
            var validUser = await _userManager.FindByEmailAsync("testuser@example.com");
            var isValidPassword = await _userManager.CheckPasswordAsync(validUser, "Test@123");

            // Assert
            Assert.NotNull(validUser);
            Assert.True(isValidPassword);
        }
    }
} 