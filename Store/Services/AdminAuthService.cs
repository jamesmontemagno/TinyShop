namespace Store.Services;

public class AdminAuthService
{
    private const string AdminPassword = "admin123"; // In production, this should be properly secured
    private bool _isAuthenticated;

    public bool IsAuthenticated => _isAuthenticated;

    public bool Login(string password)
    {
        _isAuthenticated = password == AdminPassword;
        return _isAuthenticated;
    }

    public void Logout()
    {
        _isAuthenticated = false;
    }
}