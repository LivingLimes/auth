using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using AuthServer.Application.Endpoints.PostClient;
using AuthServer.Core;
using AuthServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;

public class SignupPageTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task TestUserSignup()
    {
        var page = await _browser.NewPageAsync();
        await page.GotoAsync("http://localhost:5153/signup");

        var username = "test@example.com";
        var password = "Test1234";

        var usernameInput = await page.WaitForSelectorAsync("[data-testid='username-input']");
        if (usernameInput == null) throw new InvalidOperationException("Username input not found.");

        var passwordInput = await page.WaitForSelectorAsync("[data-testid='password-input']");
        if (passwordInput == null) throw new InvalidOperationException("Password input not found.");

        var confirmPasswordInput = await page.WaitForSelectorAsync("[data-testid='confirm-password-input']");
        if (confirmPasswordInput == null) throw new InvalidOperationException("Confirm password input not found.");

        var signupButton = await page.WaitForSelectorAsync("[data-testid='signup-button']");
        if (signupButton == null) throw new InvalidOperationException("Signup button not found.");
    
        await usernameInput.FillAsync(username);
        await passwordInput.FillAsync(password);
        await confirmPasswordInput.FillAsync(password);

        await signupButton.ClickAsync();
    
        await page.ClickAsync("[data-testid='signup-button']");
    }
} 