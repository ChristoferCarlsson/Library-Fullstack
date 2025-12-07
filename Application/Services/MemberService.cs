using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public MemberService(IMemberRepository memberRepository, IMapper mapper)
        {
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        public async Task<List<MemberDto>> GetAllAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            return _mapper.Map<List<MemberDto>>(members);
        }

        public async Task<MemberDto> GetByIdAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
                throw new NotFoundException($"Member with id {id} not found.");

            return _mapper.Map<MemberDto>(member);
        }

        public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
        {
            var member = _mapper.Map<Member>(dto);

            await _memberRepository.AddAsync(member);
            await _memberRepository.SaveChangesAsync();

            return _mapper.Map<MemberDto>(member);
        }

        public async Task<MemberDto> UpdateAsync(int id, UpdateMemberDto dto)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
                throw new NotFoundException($"Member with id {id} not found.");

            _mapper.Map(dto, member);

            _memberRepository.Update(member);
            await _memberRepository.SaveChangesAsync();

            return _mapper.Map<MemberDto>(member);
        }

        public async Task DeleteAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null)
                throw new NotFoundException($"Member with id {id} not found.");

            _memberRepository.Remove(member);
            await _memberRepository.SaveChangesAsync();
        }
    }
}
