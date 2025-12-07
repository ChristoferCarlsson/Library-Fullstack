using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

public class MemberServiceTests
{
    private readonly Mock<IMemberRepository> _repoMock;
    private readonly IMapper _mapper;
    private readonly MemberService _service;

    public MemberServiceTests()
    {
        _repoMock = new Mock<IMemberRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Application.Mapping.LibraryProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new MemberService(_repoMock.Object, _mapper, Mock.Of<ILogger<MemberService>>());
    }

    [Fact]
    public async Task GetById_ShouldThrow_WhenMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Member?)null);

        var act = async () => await _service.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_ShouldRemoveMember()
    {
        var member = new Domain.Entities.Member { Id = 1 };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(member);

        await _service.DeleteAsync(1);

        _repoMock.Verify(r => r.Remove(member), Times.Once);
    }
}
