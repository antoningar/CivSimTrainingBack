using System;
using TechTalk.SpecFlow;

namespace cst_back.specs.StepDefinitions
{
    [Binding]
    public class CreateUserStepDefinitions
    {
        [Given(@"As a user")]
        public void GivenAsAUser()
        {
            throw new PendingStepException();
        }

        [Given(@"My username is aaa")]
        public void GivenMyUsernameIsAaa()
        {
            throw new PendingStepException();
        }

        [Given(@"My email  is a@a\.com")]
        public void GivenMyEmailIsAA_Com()
        {
            throw new PendingStepException();
        }

        [Given(@"My password is aaaaaaaa")]
        public void GivenMyPasswordIsAaaaaaaa()
        {
            throw new PendingStepException();
        }

        [Given(@"My password confirmation is aaaaaaaa")]
        public void GivenMyPasswordConfirmationIsAaaaaaaa()
        {
            throw new PendingStepException();
        }

        [When(@"I want to create my  account")]
        public void WhenIWantToCreateMyAccount()
        {
            throw new PendingStepException();
        }

        [Then(@"I should receive a response  (.*)")]
        public void ThenIShouldReceiveAResponse(int p0)
        {
            throw new PendingStepException();
        }
    }
}
