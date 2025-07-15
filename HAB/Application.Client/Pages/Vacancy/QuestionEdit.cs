using System.Diagnostics.CodeAnalysis;
using Shared.Models;

namespace Application.Client.Pages.Vacancy;

    public record QuestionEdit
    {
        // public QuestionEdit()
        // {
        //     
        // }

        [SetsRequiredMembers]
        public QuestionEdit(
            Guid? id,
            string text,
            int orderNumber,
            IAnswerProperties answer)
        {
            Id = id;
            Text = text;
            OrderNumber = orderNumber;
            Answer = answer;
            Type = answer switch
            {
                TextAnswer => AnswerType.Text,
                YesNoAnswer => AnswerType.YesNo,
                _ => throw new ArgumentException("Unknown answer type", nameof(answer))
            };
        }
        
        public required Guid? Id { get; set; }
        public required string Text { get; set; } = string.Empty;
        public required int OrderNumber { get; set; }
        
        public required AnswerType Type { get; set; }

        public required IAnswerProperties Answer { get; set; }

        public void SetAnswerBasedOnType()
        {
            if (Type is AnswerType.Text && Answer is not TextAnswer)
            {
                Console.WriteLine("Changing answer type from Text to " + Type);
                Answer = new TextAnswer();
            }

            if (Type is AnswerType.YesNo && Answer is not YesNoAnswer)
            {
                Console.WriteLine("Changing answer type from YesNo to " + Type);
                Answer = new YesNoAnswer()
                {
                    HasRequiredAnswer = false,
                    RequiredAnswer = null,
                };
            }
        }

        public static QuestionEdit NewQuestion()
            => new QuestionEdit
                (
                    id: null,
                    text: string.Empty,
                    orderNumber: 1,
                    answer: new TextAnswer()
                );
        
        public static QuestionEdit NewQuestion(
            IReadOnlyCollection<QuestionEdit> existingQuestions)
            => new(
                    id: null,
                    text: string.Empty,
                    orderNumber: existingQuestions.Count + 1,
                    answer: new TextAnswer()
                );
    }

    public interface IAnswerProperties
    {
        IAnswerProperties Copy();
    }

    public record TextAnswer : IAnswerProperties
    {
        public IAnswerProperties Copy() => this with { };
    }
    
    public record YesNoAnswer : IAnswerProperties
    {
        private bool _hasRequiredAnswer;
        public required bool HasRequiredAnswer
        {
            get => _hasRequiredAnswer;
            set
            {
                Console.WriteLine("Setting HasRequiredAnswer to " + value);
                if (value && RequiredAnswer is null)
                {
                    RequiredAnswer = new RequiredAnswerProperties();
                    Console.WriteLine("RequiredAnswer was null, created new instance.");
                }
                else if (!value)
                {
                    RequiredAnswer = null;
                    Console.WriteLine("RequiredAnswer set to null.");
                }
                
                _hasRequiredAnswer = value;
                Console.WriteLine("HasRequiredAnswer is now " + value);
            }
        }
        
        public required RequiredAnswerProperties? RequiredAnswer { get; set; }


        public record RequiredAnswerProperties 
        {
            public bool ExpectedAnswer { get; set; } = true;
            public string UnexpectedAnswerRejectText { get; set; } = string.Empty;
        }

        public IAnswerProperties Copy() => this with
        {
            RequiredAnswer = RequiredAnswer is null ? null : RequiredAnswer with { }
        };
    }
