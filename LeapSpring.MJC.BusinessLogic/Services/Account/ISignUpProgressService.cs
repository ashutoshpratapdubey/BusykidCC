using LeapSpring.MJC.Core.Dto.Accounts;
using LeapSpring.MJC.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapSpring.MJC.BusinessLogic.Services.Account
{
    /// <summary>
    /// Represents a interface of sign up progress service
    /// </summary>
    public interface ISignUpProgressService
    {

        /// <summary>
        /// Update signup progress
        /// </summary>
        /// <param name="familyId">Family identifier</param>
        void UpdateSignUpProgress(int? familyId = null);

        /// <summary>
        /// Update signup progress
        /// </summary>
        /// <param name="signUpStatus">Signup status</param>
        /// <param name="familyId">Family identifier</param>
        void UpdateSignUpProgress(SignUpStatus signUpStatus, int? familyId = null);


        /// <summary>
        /// Get detailed sign up progress
        /// </summary>
        /// <returns>Sign up progress</returns>
        SignUpProgress GetDetailedSignUpProgress(int? familyId = null);

        /// <summary>
        /// Get signup progress
        /// </summary>
        /// <returns>Signup status</returns>
        SignUpStatus GetSignUpProgress();
    }
}
