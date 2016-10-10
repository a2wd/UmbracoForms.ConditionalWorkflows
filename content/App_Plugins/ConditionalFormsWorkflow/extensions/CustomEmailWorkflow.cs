using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Umbraco.Core.Configuration;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace Chillies.Handlers
{
    public class CustomEmailWorkflow : WorkflowType
    {
        private const string MessagePrevalue = @"
        A new form was submitted. The data is as follows:
        {0}
        Regards,
        The 3chillies team.";
        public CustomEmailWorkflow()
        {
            this.Name = "Send an email with conditions";
            this.Id = new Guid("638895d8-40ba-4408-b038-59e6cc801ebd");
            this.Description = "This will send an email based on a condition in the form";
            this.Icon = "icon-message";
        }

        [Setting("From Name", description = "Enter the sender's Name", view = "TextField")]
        public string FromName { get; set; }

        [Setting("From Email", description = "Enter the sender's email", view = "TextField")]
        public string FromEmail { get; set; }

        [Setting("Subject", description = "Enter a subject for the email", view = "TextField", prevalues = "A new form was submitted")]
        public string Subject { get; set; }

        [Setting("Message", description = "Enter a subject for the email, use the placeholder {0} to indicate where the form data should be inserted", view = "TextArea", prevalues = MessagePrevalue)]
        public string Message { get; set; }

        [Setting("Routes", description = "Email addresses to route the message to, depending on selected fields", view = "FieldConditions")]
        public string Routes { get; set; }

        public override WorkflowExecutionStatus Execute(Record record, RecordEventArgs e)
        {
            List<CustomEmailRouteMapping> emailRoutes = JsonConvert.DeserializeObject<List<CustomEmailRouteMapping>>(Routes);

            if(emailRoutes.Count > 0)
            {
                var message = new MailMessage();

                foreach (var route in emailRoutes)
                {
                    var fieldGuid = new Guid(route.Field);
                    if(fieldGuid != null && record.RecordFields.ContainsKey(fieldGuid) && record.RecordFields[fieldGuid].HasValue())
                    {
                        var fieldValue = record.RecordFields[fieldGuid].ValuesAsString();
                        if(!string.IsNullOrWhiteSpace(fieldValue))
                        {
                            if(fieldValue.Equals(route.FieldValue))
                            {
                                message.To.Add(route.EmailRoute);
                            }
                        }
                    }
                }

                if(message.To.Count > 0)
                {
                    if(string.IsNullOrWhiteSpace(FromEmail))
                    {
                        FromEmail = UmbracoConfig.For.UmbracoSettings().Content.NotificationEmailAddress;
                    }
                    message.From = new MailAddress(FromEmail);
                    message.Subject = Subject;
                    message.IsBodyHtml = true;


                    var messageBody = new StringBuilder();
                    messageBody.Append("<table>");
                    foreach (var field in record.RecordFields)
                    {
                        messageBody.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", FormatAlias(field.Value.Alias), field.Value.ValuesAsString());
                    }
                    messageBody.Append("</table>");

                    if(string.IsNullOrWhiteSpace(this.Message))
                    {
                        message.Body = messageBody.ToString();
                    }
                    else
                    {
                        var messageFormatted = "<p>" + this.Message.Replace("\r\n", "</p><p>").Replace("\n", "</p><p>").Replace("\r", "</p><p>") + "</p>";
                        message.Body = string.Format(messageFormatted, messageBody.ToString());
                    }

                    var client = new SmtpClient();

                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception)
                    {
                        return WorkflowExecutionStatus.Failed;
                    }
                }
            }

            return WorkflowExecutionStatus.Completed;
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptions = new List<Exception>();

            //Check email address
            var emailRegex = new Regex(@".+@.+");
            if(string.IsNullOrWhiteSpace(FromEmail) || !emailRegex.Match(FromEmail).Success)
            {
                exceptions.Add(new Exception("Please enter a valid email address"));
            }

            return exceptions;
        }

        private string FormatAlias(string alias)
        {
            if(string.IsNullOrWhiteSpace(alias))
            {
                return string.Empty;
            }

            var aliasAsCharArray = Regex.Replace(alias, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1").ToCharArray();
            aliasAsCharArray[0] = char.ToUpper(aliasAsCharArray[0]);

            return new string(aliasAsCharArray);
        }
    }
}