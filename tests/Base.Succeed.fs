module Tests.Base_Succeed

open Mocha
open Fable.Form

describe
    "Base.succeed"
    (fun () ->

        it
            "returns an empty form that always succeeds"
            (fun () ->
                let form = Base.succeed ()

                let actual = Base.fill form ()

                let expected =
                    {
                        Fields = []
                        Result = Ok()
                        IsEmpty = true
                    }
                    : Base.FilledForm<unit, obj>

                Assert.deepStrictEqual (actual, expected)
            )

    )
