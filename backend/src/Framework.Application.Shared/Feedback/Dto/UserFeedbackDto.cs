using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Framework.Feedback.Dto
{
    public class UserFeedbackDto 
    { 
        public string Email { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public string Respond { get; set; }
        public string Image { get; set; }
    }
}
