using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Instant;
using System.Linq;
using System.Linq.Expressions;

namespace RadicalR
{
    public class DsoCommandValidator<TEntity> : CommandValidatorBase<DsoCommand<TEntity>> where TEntity : Entity
    {
        public DsoCommandValidator(IServicer ultimate) : base(ultimate) 
        { }

        protected void ValidateNotEmpty(Expression<Func<DsoCommand<TEntity>, object>> member)
        {
            RuleFor(member)
                .NotEmpty().WithMessage(
                    $"{member.Parameters.FirstOrDefault().GetType().GetGenericArguments().FirstOrDefault().Name} is required!");
        }

        protected void ValidateNotEmpty(params string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                RuleFor(a => a.Data.ValueOf(propertyName))
                    .NotEmpty().WithMessage(a => 
                        $"{propertyName} is required!");
            }
        }

        protected void ValidateEmail(Expression<Func<DsoCommand<TEntity>, string>> member)
        {
            RuleFor(member)
                    .EmailAddress()
                    .When(a => !string.IsNullOrEmpty(a.Data.ValueOf(member.Parameters.FirstOrDefault().GetType().GetGenericArguments().FirstOrDefault().Name).ToString()))
                    .WithMessage($"Invalid email address.");
        }

        protected void ValidateLength(int min, int max, Expression<Func<DsoCommand<TEntity>, string>> member)
        {
            RuleFor(member)
                .MinimumLength(min)
                .WithMessage($"minimum text length: {max} characters")
                .MaximumLength(max)
                .WithMessage($"maximum text length: {max} characters");
        }

        protected void ValidateEnum(Expression<Func<DsoCommand<TEntity>, string>> member)
        {
            RuleFor(member)
                .IsInEnum().WithMessage($"Incorrect enum value");
        }

        protected void ValidateNotEqual(object item, Expression<Func<DsoCommand<TEntity>, object>> member)
        {
            RuleFor(member)
                .NotEqual(item).WithMessage($"value not equal: {item}");
        }

        protected void ValidateLanguage(Expression<Func<DsoCommand<TEntity>, object>> member)
        {
            RuleFor(member)
                .Must(SupportedLanguages.Contains).WithMessage("Agreement language must conform to ISO 639-1.");
        }

        protected void ValidateExist<TStore>(Func<DsoCommand<TEntity>, Expression<Func<TEntity, bool>>> command) where TStore : IDataStore            
        {
            RuleFor(e => e)
                .MustAsync(async (cmd, cancel) =>
                {
                    Expression<Func<TEntity, bool>> predicate = command(cmd);
                    return await uservice.use<TStore, TEntity>().Exist(predicate);

                }).WithMessage($"{typeof(TEntity).Name} does not exists");            
        }

        protected void ValidateNotExist<TStore>(Func<DsoCommand<TEntity>, Expression<Func<TEntity, bool>>> command) where TStore : IDataStore          
        {
            RuleFor(e => e)
                .MustAsync(async (cmd, cancel) =>
                {
                    Expression<Func<TEntity, bool>> predicate = command(cmd);
                    return await uservice.use<TStore, TEntity>().NotExist(predicate);

                }).WithMessage($"{typeof(TEntity).Name} already exists");
        }     
    }
}