using Microsoft.AspNetCore.Identity;

namespace StepApi.Identity.Model.OutputModel
{
    public class ResponseRegistration : BaseResponse
    {
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
