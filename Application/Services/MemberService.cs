using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MemberService> _logger;

        public MemberService(
            IMemberRepository memberRepository,
            IMapper mapper,
            ILogger<MemberService> logger)
        {
            _memberRepository = memberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<MemberDto>> GetAllAsync()
        {
            var members = await _memberRepository.GetAllAsync();

            _logger.LogInformation("Fetched {Count} members", members.Count);

            return _mapper.Map<List<MemberDto>>(members);
        }

        public async Task<MemberDto> GetByIdAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);

            if (member == null)
            {
                _logger.LogWarning("Member with ID {Id} not found", id);
                throw new NotFoundException($"Member with id {id} not found.");
            }

            _logger.LogInformation("Fetched member with ID {Id}", id);

            return _mapper.Map<MemberDto>(member);
        }

        public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
        {
            var member = _mapper.Map<Member>(dto);

            await _memberRepository.AddAsync(member);
            await _memberRepository.SaveChangesAsync();

            _logger.LogInformation("Created member with ID {Id}", member.Id);

            return _mapper.Map<MemberDto>(member);
        }

        public async Task<MemberDto> UpdateAsync(int id, UpdateMemberDto dto)
        {
            var member = await _memberRepository.GetByIdAsync(id);

            if (member == null)
            {
                _logger.LogWarning("Attempted update: Member with ID {Id} not found", id);
                throw new NotFoundException($"Member with id {id} not found.");
            }

            _mapper.Map(dto, member);

            _memberRepository.Update(member);
            await _memberRepository.SaveChangesAsync();

            _logger.LogInformation("Updated member with ID {Id}", id);

            return _mapper.Map<MemberDto>(member);
        }

        public async Task DeleteAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);

            if (member == null)
            {
                _logger.LogWarning("Attempted delete: Member with ID {Id} not found", id);
                throw new NotFoundException($"Member with id {id} not found.");
            }

            _memberRepository.Remove(member);
            await _memberRepository.SaveChangesAsync();

            _logger.LogWarning("Deleted member with ID {Id}", id);
        }
    }
}
