using System;
using Dapper.Contrib.Extensions;

namespace Modules
{
  public abstract class User
  {
    [Key]
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime? Birthday { get; set; }
    public string Mail { get; set; }
    public string PassWord { get; set; }
    public byte[] Image { get; set; }
    public string ImageTitle { get; set; }
  }
}
