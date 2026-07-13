using Cartographer.Core.Abstractions;
using Cartographer.Core.DependencyInjection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Common.Behaviors;
using TodoApp.Application.LunchPreferences.Commands.CreateOrUpdateLunchPreference;
using TodoApp.Application.LunchPreferences.Dtos;
using TodoApp.Application.Todos.Dtos;
using TodoApp.Domain.Entities;

namespace TodoApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddCartographer(cfg =>
        {
            cfg.CreateMap<TodoItem, TodoItemResponse>()
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            cfg.CreateMap<Priority, PriorityResponse>();
            cfg.CreateMap<Status, StatusResponse>();
            cfg.CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.TodoCount, opt => opt.MapFrom(src => src.TodoItems.Count));
            cfg.CreateMap<CreateTodoRequest, TodoItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
            cfg.CreateMap<UpdateTodoRequest, TodoItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Priority, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            cfg.CreateMap<LunchPreference, LunchPreferenceResponse>();
            cfg.CreateMap<UpdateLunchPreferenceRequest, LunchPreference>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            cfg.CreateMap<CreateOrUpdateLunchPreferenceCommand, LunchPreference>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        });

        return services;
    }
}
