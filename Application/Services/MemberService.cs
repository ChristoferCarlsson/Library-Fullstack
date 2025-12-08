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

            var dtoList = _mapper.Map<List<MemberDto>>(members);

            foreach (var dto in dtoList)
            {
                var member = members.First(m => m.Id == dto.Id);
                dto.LoanCount = member.Loans.Count(l => l.ReturnDate == null);
            }

            return dtoList;
        }

        public async Task<MemberDto> GetByIdAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);

            if (member == null)
            {
                _logger.LogWarning("Member with ID {Id} not found", id);
                throw new NotFoundException($"Member with id {id} not found.");
            }

            var dto = _mapper.Map<MemberDto>(member);
            dto.LoanCount = member.Loans.Count(l => l.ReturnDate == null);

            _logger.LogInformation("Fetched member with ID {Id}", id);

            return dto;
        }

        public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
        {
            var member = _mapper.Map<Member>(dto);

            await _memberRepository.AddAsync(member);
            await _memberRepository.SaveChangesAsync();

            _logger.LogInformation("Created member with ID {Id}", member.Id);

            var result = _mapper.Map<MemberDto>(member);
            result.LoanCount = 0;
            return result;
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

            var result = _mapper.Map<MemberDto>(member);
            result.LoanCount = member.Loans.Count(l => l.ReturnDate == null);

            _logger.LogInformation("Updated member with ID {Id}", id);

            return result;
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
