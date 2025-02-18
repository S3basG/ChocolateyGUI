using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using ChocolateyGui.Common.Windows.ViewModels.Items;
using ChocolateyGui.Common.Services;
using ChocolateyGui.Common.Models.Messages;
using ChocolateyGui.Common.Windows.Services;
using AutoMapper;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using NuGet.Versioning;

namespace ChocolateyGui.Tests.ViewModels
{
    public class PackageViewModelTests
    {
        private readonly Mock<IChocolateyService> _chocolateyServiceMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<IProgressService> _progressServiceMock;
        private readonly Mock<IChocolateyGuiCacheService> _chocolateyGuiCacheServiceMock;
        private readonly Mock<IConfigService> _configServiceMock;
        private readonly Mock<IAllowedCommandsService> _allowedCommandsServiceMock;
        private readonly Mock<IPackageArgumentsService> _packageArgumentsServiceMock;
        private readonly Mock<IPersistenceService> _persistenceServiceMock;

        public PackageViewModelTests()
        {
            _chocolateyServiceMock = new Mock<IChocolateyService>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _mapperMock = new Mock<IMapper>();
            _dialogServiceMock = new Mock<IDialogService>();
            _progressServiceMock = new Mock<IProgressService>();
            _chocolateyGuiCacheServiceMock = new Mock<IChocolateyGuiCacheService>();
            _configServiceMock = new Mock<IConfigService>();
            _allowedCommandsServiceMock = new Mock<IAllowedCommandsService>();
            _packageArgumentsServiceMock = new Mock<IPackageArgumentsService>();
            _persistenceServiceMock = new Mock<IPersistenceService>();
        }

        [Fact]
        public async Task Reinstall_ShouldShowConfirmationMessage_WhenNewerVersionIsInstalled()
        {
            // Arrange
            var viewModel = new PackageViewModel(
                _chocolateyServiceMock.Object,
                _eventAggregatorMock.Object,
                _mapperMock.Object,
                _dialogServiceMock.Object,
                _progressServiceMock.Object,
                _chocolateyGuiCacheServiceMock.Object,
                _configServiceMock.Object,
                _allowedCommandsServiceMock.Object,
                _packageArgumentsServiceMock.Object,
                _persistenceServiceMock.Object)
            {
                Id = "testPackage",
                Version = new NuGetVersion("1.0.0")
            };

            var installedPackage = new Package { Version = new NuGetVersion("2.0.0") };
            _chocolateyServiceMock.Setup(s => s.GetByVersionAndIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(installedPackage);

            _dialogServiceMock.Setup(d => d.ShowConfirmationMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(MessageDialogResult.Affirmative);

            // Act
            await viewModel.Reinstall();

            // Assert
            _dialogServiceMock.Verify(d => d.ShowConfirmationMessageAsync(
                It.Is<string>(s => s == "Are you sure?"),
                It.Is<string>(s => s.Contains("You currently have a newer version of the 'testPackage' package installed"))), Times.Once);
        }

        [Fact]
        public async Task Reinstall_ShouldShowConfirmationMessage_WhenNoNewerVersionIsInstalled()
        {
            // Arrange
            var viewModel = new PackageViewModel(
                _chocolateyServiceMock.Object,
                _eventAggregatorMock.Object,
                _mapperMock.Object,
                _dialogServiceMock.Object,
                _progressServiceMock.Object,
                _chocolateyGuiCacheServiceMock.Object,
                _configServiceMock.Object,
                _allowedCommandsServiceMock.Object,
                _packageArgumentsServiceMock.Object,
                _persistenceServiceMock.Object)
            {
                Id = "testPackage",
                Version = new NuGetVersion("2.0.0")
            };

            var installedPackage = new Package { Version = new NuGetVersion("1.0.0") };
            _chocolateyServiceMock.Setup(s => s.GetByVersionAndIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(installedPackage);

            _dialogServiceMock.Setup(d => d.ShowConfirmationMessageAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(MessageDialogResult.Affirmative);

            // Act
            await viewModel.Reinstall();

            // Assert
            _dialogServiceMock.Verify(d => d.ShowConfirmationMessageAsync(
                It.Is<string>(s => s == "Are you sure?"),
                It.Is<string>(s => s.Contains("This action will attempt to reinstall the 'testPackage' package"))), Times.Once);
        }
    }
}