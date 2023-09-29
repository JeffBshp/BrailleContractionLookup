namespace BrailleContractions.Services
{
    /// <summary>
    /// Provides platform-specific info.
    /// </summary>
    public interface ISystemService
    {
        string AppVersion { get; }

        FontScale FontScale { get; }
    }

    /// <summary>
    /// Platforms provide a global font scaling feature for accessibility.
    /// The platform-specific values are converted to this enum,
    /// making them easier for this app to handle.
    /// The integer values are used as the RowHeight of the ListView
    /// (see <see cref="ViewModels.LookupPageVM"/>).
    /// </summary>
    public enum FontScale
    {
        Small = 40,
        Medium = 48,
        Large = 56,
        ExtraLarge = 64
    }
}
