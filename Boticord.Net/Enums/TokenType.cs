namespace Boticord.Net.Enums;


/// <summary>
/// Enum representing type of the supplied token (if any)
/// </summary>
public enum TokenType
{
    /// <summary>
    /// No token is supplied
    /// </summary>
    None,

    /// <summary>
    /// Bot token
    /// </summary>
    Bot,

    /// <summary>
    /// PrivateBot token
    /// </summary>
    PrivateBot,

    /// <summary>
    /// Profile token
    /// </summary>
    Profile
}