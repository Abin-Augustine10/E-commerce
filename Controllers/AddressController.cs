using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopZone.DTOs.Request.Address;
using ShopZone.DTOs.Response.Address;
using ShopZone.DTOs.Response.Common;
using ShopZone.Models;
using ShopZone.Services.Interfaces;
using System.Security.Claims;

namespace ShopZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Buyer")]
    public class AddressController : ControllerBase
    {
        private readonly IGenericRepository<DeliveryAddress> _addressRepository;

        public AddressController(IGenericRepository<DeliveryAddress> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AddressResponseDto>>>> GetAddresses()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var addresses = await _addressRepository.GetAllAsync(a => a.UserId == userId);

                var addressDtos = addresses.Select(address => new AddressResponseDto
                {
                    Id = address.Id,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    Pincode = address.Pincode,
                    IsDefault = address.IsDefault,
                    CreatedAt = address.CreatedAt
                }).ToList();

                return Ok(ApiResponse<List<AddressResponseDto>>.SuccessResponse(addressDtos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<List<AddressResponseDto>>.ErrorResponse("Failed to get addresses"));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> GetAddress(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var address = await _addressRepository.GetByIdAsync(id);

                if (address == null || address.UserId != userId)
                {
                    return NotFound(ApiResponse<AddressResponseDto>.ErrorResponse("Address not found"));
                }

                var addressDto = new AddressResponseDto
                {
                    Id = address.Id,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    Pincode = address.Pincode,
                    IsDefault = address.IsDefault,
                    CreatedAt = address.CreatedAt
                };

                return Ok(ApiResponse<AddressResponseDto>.SuccessResponse(addressDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AddressResponseDto>.ErrorResponse("Failed to get address"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> CreateAddress([FromBody] CreateAddressRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                // If this is set as default, make all other addresses non-default
                if (request.IsDefault)
                {
                    var existingAddresses = await _addressRepository.GetAllAsync(a => a.UserId == userId && a.IsDefault);
                    foreach (var existingAddress in existingAddresses)
                    {
                        existingAddress.IsDefault = false;
                        await _addressRepository.UpdateAsync(existingAddress);
                    }
                }

                var address = new DeliveryAddress
                {
                    UserId = userId,
                    Street = request.Street,
                    City = request.City,
                    State = request.State,
                    Pincode = request.Pincode,
                    IsDefault = request.IsDefault
                };

                var createdAddress = await _addressRepository.AddAsync(address);

                var addressDto = new AddressResponseDto
                {
                    Id = createdAddress.Id,
                    Street = createdAddress.Street,
                    City = createdAddress.City,
                    State = createdAddress.State,
                    Pincode = createdAddress.Pincode,
                    IsDefault = createdAddress.IsDefault,
                    CreatedAt = createdAddress.CreatedAt
                };

                return Ok(ApiResponse<AddressResponseDto>.SuccessResponse(addressDto, "Address created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AddressResponseDto>.ErrorResponse("Failed to create address"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> UpdateAddress(int id, [FromBody] UpdateAddressRequestDto request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var address = await _addressRepository.GetByIdAsync(id);

                if (address == null || address.UserId != userId)
                {
                    return NotFound(ApiResponse<AddressResponseDto>.ErrorResponse("Address not found"));
                }

                // If this is set as default, make all other addresses non-default
                if (request.IsDefault && !address.IsDefault)
                {
                    var existingAddresses = await _addressRepository.GetAllAsync(a => a.UserId == userId && a.IsDefault);
                    foreach (var existingAddress in existingAddresses)
                    {
                        existingAddress.IsDefault = false;
                        await _addressRepository.UpdateAsync(existingAddress);
                    }
                }

                address.Street = request.Street;
                address.City = request.City;
                address.State = request.State;
                address.Pincode = request.Pincode;
                address.IsDefault = request.IsDefault;
                address.UpdatedAt = DateTime.UtcNow;

                await _addressRepository.UpdateAsync(address);

                var addressDto = new AddressResponseDto
                {
                    Id = address.Id,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    Pincode = address.Pincode,
                    IsDefault = address.IsDefault,
                    CreatedAt = address.CreatedAt
                };

                return Ok(ApiResponse<AddressResponseDto>.SuccessResponse(addressDto, "Address updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AddressResponseDto>.ErrorResponse("Failed to update address"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteAddress(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var address = await _addressRepository.GetByIdAsync(id);

                if (address == null || address.UserId != userId)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse("Address not found"));
                }

                await _addressRepository.DeleteAsync(id);

                return Ok(ApiResponse<string>.SuccessResponse("", "Address deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Failed to delete address"));
            }
        }
    }
}