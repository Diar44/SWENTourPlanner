using NUnit.Framework;
using SWENTourPlanner.Models;
using SWENTourPlanner.ViewModels;
using System.Collections.ObjectModel;

namespace SWENTourPlanner.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel;

        [SetUp]
        public void SetUp()
        {
            _mainViewModel = new MainViewModel();
        }

        [Test]
        public void EditTour_ShouldNotThrowExceptionIfNoTourSelected()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _mainViewModel.EditTourCommand.Execute(null));
        }

        [Test]
        public void DeleteTour_ShouldNotThrowExceptionIfNoTourSelected()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _mainViewModel.DeleteTourCommand.Execute(null));
        }

        [Test]
        public void OpenTourLogs_ShouldNotThrowExceptionIfNoTourSelected()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _mainViewModel.ShowTourLog.Execute(null));
        }

        [Test]
        public void DeleteTour_ShouldDoNothingIfNoTourSelected()
        {
            // Arrange
            var initialCount = _mainViewModel.Tours.Count;

            // Act
            _mainViewModel.DeleteTourCommand.Execute(null);

            // Assert
            Assert.AreEqual(initialCount, _mainViewModel.Tours.Count);
        }
    }
}