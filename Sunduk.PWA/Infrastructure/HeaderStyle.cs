namespace Sunduk.PWA.Infrastructure
{
    /// <summary>
    /// Формат строки с номером программы в шапке — два формата, поддерживаемых Fanuc.
    /// ONumber — "O0001". AngleBracketName — "&lt;имя&gt;".
    /// </summary>
    public enum HeaderStyle
    {
        ONumber,
        AngleBracketName,
    }
}
