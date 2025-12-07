using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class LibraryProfile : Profile
    {
        public LibraryProfile()
        {
            // Author
            CreateMap<Author, AuthorDto>();
            CreateMap<CreateAuthorDto, Author>();
            CreateMap<UpdateAuthorDto, Author>();

            // Book
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.AuthorFullName,
                    opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));

            CreateMap<CreateBookDto, Book>()
                .ForMember(dest => dest.CopiesAvailable,
                    opt => opt.MapFrom(src => src.CopiesTotal)); // when creating a new book, available = total

            CreateMap<UpdateBookDto, Book>();

            // Member
            CreateMap<Member, MemberDto>();
            CreateMap<CreateMemberDto, Member>();
            CreateMap<UpdateMemberDto, Member>();

            // Loan
            CreateMap<Loan, LoanDto>();
            CreateMap<CreateLoanDto, Loan>();
            CreateMap<UpdateLoanDto, Loan>();
        }
    }
}
