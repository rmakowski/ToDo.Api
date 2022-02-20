using System.Xml;
using ToDo.API.Entities;
using ToDo.API.Interfaces;

namespace ToDo.API.Models.Responses;

public class LoginResponse : IObjectResponse<User, LoginResponse>
{
    /// <example>Login</example>>
    public string Login { get; set; } = null!;

    /// <example>2022-02-20T20:40:31.459838Z</example>>
    public string CreatedDate { get; set; } = null!;

    /// <example>2022-02-20T20:40:31.459838Z</example>>
    public string LastLoginDate { get; set; } = null!;

    public string Token { get; set; } = null!;

    public List<GetToDoItemsResponse> ToDoItems { get; set; } = null!;

    public static Func<User, LoginResponse> Map
    {
        get
        {
            return user => new LoginResponse
            {
                Login = user.Login,
                CreatedDate = XmlConvert.ToString(user.CreatedDate, XmlDateTimeSerializationMode.Utc),
                LastLoginDate = XmlConvert.ToString(user.LastLoginDate, XmlDateTimeSerializationMode.Utc),
                ToDoItems = user.ToDoItems.Select(GetToDoItemsResponse.Map).ToList()
            };
        }
    }
}
