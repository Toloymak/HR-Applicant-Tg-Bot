using Shared.Models;

namespace Application.Client.Pages.Vacancy;

    public record QuestionEdit
    {
        public required Guid? Id { get; set; }
        public required string Text { get; set; } = string.Empty;
        
        private AnswerType _type;
        public required AnswerType Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
                    Answer = value switch
                    {
                        AnswerType.Text => new TextAnswer(),
                        AnswerType.YesNo => new YesNoAnswer(),
                        _ => throw new NotSupportedException($"Answer type {value} is not supported.")
                    };

                    _type = value;
                }
            }
        }

        public IAnswerProperties Answer { get; set; } = new TextAnswer();

        public static QuestionEdit NewQuestion()
            => new QuestionEdit
            {
                Id = null,
                Text = string.Empty,
                Type = AnswerType.Text,
                Answer = new TextAnswer(),
            };
    }

    public interface IAnswerProperties
    {
        
    }

    public record TextAnswer : IAnswerProperties
    {
        
    }
    
    public record YesNoAnswer : IAnswerProperties
    {
        private bool _hasRequiredAnswer;
        public bool HasRequiredAnswer
        {
            get => _hasRequiredAnswer;
            set
            {
                if (value && !_hasRequiredAnswer)
                    RequiredAnswer = new RequiredAnswerProperties();
                
                _hasRequiredAnswer = value;
            }
        }
        
        public RequiredAnswerProperties? RequiredAnswer { get; set; }


        public record RequiredAnswerProperties 
        {
            public bool ExpectedAnswer { get; set; } = true;
            public string UnexpectedAnswerRejectText { get; set; } = string.Empty;
        }
    }
