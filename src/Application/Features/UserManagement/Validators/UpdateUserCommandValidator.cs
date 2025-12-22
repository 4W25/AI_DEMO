using FluentValidation;
using AI_DEMO.Domain.Enums;
using AI_DEMO.Application.Features.UserManagement.Commands;

namespace AI_DEMO.Application.Features.UserManagement.Validators;

/// <summary>
/// 更新使用者命令驗證器，負責驗證 UpdateUserCommand 的輸入資料。
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>
    /// 初始化 UpdateUserCommandValidator 的新實例，並設定驗證規則。
    /// </summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("使用者 ID 不可為空。");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("使用者名稱不可為空。")
            .Length(3, 50).WithMessage("使用者名稱長度必須在 3 到 50 之間。")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("使用者名稱僅允許英數字與底線。");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("電子郵件不可為空。")
            .EmailAddress().WithMessage("電子郵件格式不正確。")
            .MaximumLength(100).WithMessage("電子郵件長度不可超過 100。");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("角色必須是有效的使用者角色。");
    }
}