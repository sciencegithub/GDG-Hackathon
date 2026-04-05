namespace Backend.Services.Interfaces;

using Backend.Models.DTOs;
using Backend.Models.Entities;

public interface IUserService
{
    Task<List<UserDto>> GetAll();
    Task<UserDto> GetById(Guid userId);
}