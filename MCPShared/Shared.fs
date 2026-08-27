namespace MCPShared

open System

type Person =
    { Name: string
      DateOfBirth: DateOnly }

    member this.Age(?atTime: DateTime) =
        let dobAsTime = this.DateOfBirth.ToDateTime(TimeOnly.MinValue)
        let calculateTo = defaultArg atTime DateTime.Now
        let years = calculateTo.Year - dobAsTime.Year

        if calculateTo.Date < dobAsTime.Date.AddYears(years) then
            years - 1
        else
            years


type StoredItem<'k, 'v when 'k: comparison> = { Id: 'k; Value: 'v }

type Age = { Age: int }
