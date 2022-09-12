namespace Modules
{
  public static class Global
  {
    const string Server = @"w0605\sqlexpress";
    const string _connstring = @"Data Source=Server;Initial Catalog=Base;Integrated Security=true;MultipleActiveResultSets=true;";
    public static readonly string connectionString = _connstring.Replace("Server",Server).Replace("Base","School");
  }
}