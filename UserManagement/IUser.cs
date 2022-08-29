namespace UserManagement
{
    public interface IUser
    {
        string Address { get; set; }
        string Email { get; set; }
        decimal Money { get; set; }
        string Name { get; set; }
        string Phone { get; set; }
        string UserType { get; set; }
    }
}