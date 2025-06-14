using System;
using System.ComponentModel.DataAnnotations;

public class DiagnosisCreateDto
{
    [Required]
    public string SessionId { get; set; }

    [Required]
    public string Result { get; set; }

    [Required]
    public int DiagnosticRuleId { get; set; }

    public string Code { get; set; }
    public string Title { get; set; }

    // بهتر است CreatedAt را در سمت کلاینت نفرستید و در سرور ست کنید
    // پس اینجا Nullable می‌کنیم یا حذفش می‌کنیم
    public DateTime? CreatedAt { get; set; }

    [Required]
    public string UserId { get; set; }
}
