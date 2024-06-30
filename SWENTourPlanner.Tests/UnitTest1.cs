using NUnit.Framework.Internal;
using NUnit.Framework.Legacy;
using SWENTourPlanner.Models;
using SWENTourPlanner.ViewModels;
using System;
using System.Collections.ObjectModel;

namespace SWENTourPlanner.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private MainViewModel _mainViewModel;
        private TourDataViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _mainViewModel = new MainViewModel();
            _viewModel = new TourDataViewModel();
        }

        [Test]
        public void EditTour_ShouldNotThrowExceptionIfNoTourSelected()
        {
            // Act & Assert
            ClassicAssert.DoesNotThrow(() => _mainViewModel.EditTourCommand.Execute(null));
        }

        [Test]
        public void DeleteTour_ShouldNotThrowExceptionIfNoTourSelected()
        {
            // Act & Assert
            ClassicAssert.DoesNotThrow(() => _mainViewModel.DeleteTourCommand.Execute(null));
        }

        [Test]
        public void OpenTourLogs_ShouldNotThrowExceptionIfNoTourSelected()
        {
            // Act & Assert
            ClassicAssert.DoesNotThrow(() => _mainViewModel.ShowTourLog.Execute(null));
        }

        [Test]
        public void Constructor_InitializesProperties()
        {
            // Assert
            ClassicAssert.IsNotNull(_viewModel.AvailableLocations);
            ClassicAssert.IsNull(_viewModel.Name);
            ClassicAssert.IsNull(_viewModel.From);
            ClassicAssert.IsNull(_viewModel.To);
            ClassicAssert.IsNull(_viewModel.Description);
            ClassicAssert.IsNull(_viewModel.WebViewSource);
            ClassicAssert.IsNotNull(_viewModel.SubmitCommand);
            ClassicAssert.IsNotNull(_viewModel.LoadMapCommand);
        }

        [Test]
        public void SubmitCommand_ShouldInvokeDataSubmittedEvent()
        {
            // Arrange
            bool eventRaised = false;
            _viewModel.DataSubmitted += (sender, args) => { eventRaised = true; };

            // Act
            _viewModel.Name = "Test Tour";
            _viewModel.From = "City A";
            _viewModel.To = "City B";
            _viewModel.SubmitCommand.Execute(null);

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void GeocodeOfCity_ShouldReturnValidCoordinates()
        {
            // Arrange
            string city = "Berlin";

            // Act
            var coordinates = _viewModel.GeocodeOfCity(city).Result;

            // Assert
            ClassicAssert.IsNotNull(coordinates);
            ClassicAssert.IsTrue(coordinates.Contains(","));
        }

        [Test]
        public void GeocodeOfCity_ShouldHandleInvalidCity()
        {
            // Arrange
            string city = "InvalidCityName";

            // Act
            var coordinates = _viewModel.GeocodeOfCity(city).Result;

            // Assert
            ClassicAssert.IsEmpty(coordinates);
        }

        [Test]
        public void Name_PropertyChanged_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name")
                    eventRaised = true;
            };

            // Act
            _viewModel.Name = "Test Name";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void From_PropertyChanged_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "From")
                    eventRaised = true;
            };

            // Act
            _viewModel.From = "City A";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void To_PropertyChanged_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "To")
                    eventRaised = true;
            };

            // Act
            _viewModel.To = "City B";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void Description_PropertyChanged_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Description")
                    eventRaised = true;
            };

            // Act
            _viewModel.Description = "Test Description";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void ErrorHandling_ShouldShowErrorMessageOnSubmitError()
        {
            // Arrange
            string errorMessage = string.Empty;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Name")
                    errorMessage = _viewModel["Name"];
            };

            // Act
            _viewModel.Name = string.Empty;

            // Assert
            ClassicAssert.AreEqual("Name is required.", errorMessage);
        }


        [Test]
        public void SubmitCommand_CanExecute_WhenFieldsEmpty()
        {
            // Arrange (SetUp method initializes _viewModel)

            // Act
            _viewModel.Name = "";
            _viewModel.From = "";
            _viewModel.To = "";

            // Assert
            ClassicAssert.IsFalse(_viewModel.SubmitCommand.CanExecute(null));
        }

        [Test]
        public void SubmitCommand_CanExecute_WhenFieldsFilled()
        {
            // Arrange (SetUp method initializes _viewModel)

            // Act
            _viewModel.Name = "Test Tour";
            _viewModel.From = "City A";
            _viewModel.To = "City B";

            // Assert
            ClassicAssert.IsTrue(_viewModel.SubmitCommand.CanExecute(null));
        }


        [Test]
        public void LoadMapCommand_CanExecute_AlwaysTrue()
        {
            // Arrange & Act (SetUp method initializes _viewModel)

            // Assert
            ClassicAssert.IsTrue(_viewModel.LoadMapCommand.CanExecute(null));
        }

        [Test]
        public void PropertyChangedEvent_FiresForName()
        {
            // Arrange (SetUp method initializes _viewModel)
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.Name))
                    eventRaised = true;
            };

            // Act
            _viewModel.Name = "New Name";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void PropertyChangedEvent_FiresForFrom()
        {
            // Arrange (SetUp method initializes _viewModel)
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.From))
                    eventRaised = true;
            };

            // Act
            _viewModel.From = "New From";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void PropertyChangedEvent_FiresForTo()
        {
            // Arrange (SetUp method initializes _viewModel)
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.To))
                    eventRaised = true;
            };

            // Act
            _viewModel.To = "New To";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void PropertyChangedEvent_FiresForDescription()
        {
            // Arrange (SetUp method initializes _viewModel)
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.Description))
                    eventRaised = true;
            };

            // Act
            _viewModel.Description = "New Description";

            // Assert
            ClassicAssert.IsTrue(eventRaised);
        }

        [Test]
        public void PropertyChangedEvent_DoesNotFireForNonExistingProperty()
        {
            // Arrange (SetUp method initializes _viewModel)
            bool eventRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "NonExistingProperty")
                    eventRaised = true;
            };

            // Act
            _viewModel.Name = "Test Name";

            // Assert
            ClassicAssert.IsFalse(eventRaised);
        }

        [Test]
        public void ErrorProperty_WhenNameIsInvalid()
        {
            // Arrange (SetUp method initializes _viewModel)

            // Act
            _viewModel.Name = "";

            // Assert
            ClassicAssert.IsNotEmpty(_viewModel["Name"]);
        }

        [Test]
        public void ErrorProperty_WhenFromIsInvalid()
        {
            // Arrange (SetUp method initializes _viewModel)

            // Act
            _viewModel.From = "";

            // Assert
            ClassicAssert.IsNotEmpty(_viewModel["From"]);
        }

        [Test]
        public void ErrorProperty_WhenToIsInvalid()
        {
            // Arrange (SetUp method initializes _viewModel)

            // Act
            _viewModel.To = "";

            // Assert
            ClassicAssert.IsNotEmpty(_viewModel["To"]);
        }

        [Test]
        public void ErrorProperty_ReturnsEmptyForValidInput()
        {
            // Arrange (SetUp method initializes _viewModel)

            // Act
            _viewModel.Name = "Valid Name";
            _viewModel.From = "Valid From";
            _viewModel.To = "Valid To";

            // Assert
            ClassicAssert.IsEmpty(_viewModel["Name"]);
            ClassicAssert.IsEmpty(_viewModel["From"]);
            ClassicAssert.IsEmpty(_viewModel["To"]);
        }
    }
}
