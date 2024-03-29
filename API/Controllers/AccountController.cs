using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> getCurrentUser()
        {
            var user = await _userManager.findByEmailFromClaimsPrincipal(User);

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> checkIfEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressToReturnDto>> getUserAddress()
        {
            var user = await _userManager.findUserByClaimsPrincipalWithAddressAsync(User);
            Console.WriteLine("user ici pour check " + user);

            return _mapper.Map<Address, AddressToReturnDto>(user.Address);
        }

        //[Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressToReturnDto>> updateUserAddress(AddressToReturnDto address)
        {
            var user = await _userManager.findUserByClaimsPrincipalWithAddressAsync(User);

            user.Address = _mapper.Map<AddressToReturnDto, Address>(address);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(_mapper.Map<Address, AddressToReturnDto>(user.Address));
            }

            return BadRequest("Problem updating the user");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> register(RegisterDto registerDto)
        {
            var registeredUser = new AppUser
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(registeredUser, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }

            return new UserDto
            {
                Email = registeredUser.Email,
                Token = _tokenService.CreateToken(registeredUser),
                DisplayName = registeredUser.DisplayName
            };
        }
    }
}