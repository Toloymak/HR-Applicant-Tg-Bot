# Candidate Telegram Bot — Buttons-Only Flow

This diagram shows only user flows triggered by inline buttons (callback queries). Text commands and free-text answers are excluded, except where a button initiates text input.

```mermaid
stateDiagram-v2
    %% Entry points in UI
    [*] --> VacancyList

    %% Vacancy browsing
    VacancyList --> VacancyInfo: VacancyInfoCallback
    VacancyList --> VacancyList: UpdateVacancyListCallback
    VacancyInfo --> ApplyForVacancy: ApplyForVacancyCallback
    VacancyInfo --> VacancyList: ShowOtherVacanciesCallback

    %% Start application and Q&A via buttons
    ApplyForVacancy --> InProgress
    InProgress --> QuestionPrompt
    QuestionPrompt --> AnswerYes: AnswerYesCallback
    QuestionPrompt --> AnswerNo: AnswerNoCallback
    QuestionPrompt --> AwaitText: AnswerTextCallback
    
    %% Yes/No loops to next question
    AnswerYes --> QuestionPrompt
    AnswerNo --> QuestionPrompt
    
    %% Text path leaves button-only flow (user types message)
    AwaitText --> [*]

    %% Cancel active application (global Cancel button)
    QuestionPrompt --> CancelConfirm: CancelApplicationCallback
    VacancyList --> CancelConfirm: CancelApplicationCallback
    CancelConfirm --> Canceled: ConfirmCancelApplicationCallback
    Canceled --> VacancyList

    %% Status and revoke (from lists or status entry)
    VacancyList --> StatusSummary: ShowStatusCallback
    StatusSummary --> RevokeMenu: ShowRevokeListCallback
    RevokeMenu --> RevokeSpecific: RevokeSpecificApplicationCallback
    RevokeSpecific --> Revoked: ConfirmRevokeApplicationCallback
    Revoked --> StatusSummary
    RevokeSpecific --> StatusSummary: CancelRevokeApplicationCallback

    %% Start new flow buttons
    StatusSummary --> VacancyList: StartNewApplicationCallback

    %% Post-completion CTA (after finishing questions)
    QuestionPrompt --> SendOneMore: (final step shows button)
    SendOneMore --> VacancyList: SendOneMoreApplicationCallback
```

Callback coverage in this flow:
- Vacancy listing: `vacancy_info`, `update_vacancies`, `show_others`
- Application start: `apply_for_vacancy`
- Answering: `answer_yes`, `answer_no`, `answer_text` (initiates text input)
- Cancel application: `cancel_app`, `confirm_cancel_app`
- Status and revoke: `show_status`, `revoke_list`, `revoke_specific`, `confirm_revoke_app`, `cancel_revoke_app`
- Start new / after complete: `start_new_app`, `send_one_more`

