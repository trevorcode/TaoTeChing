module TaoTeChing.Views

open Giraffe.ViewEngine
open TaoTeChing.Types

let baseTag = voidTag "base"

let renderLayout titleText metaDescription page =
    html [ _lang "en" ] [
        head [] [
            title [] [ encodedText titleText ]
            link [ _rel "stylesheet"
                   _type "text/css"
                   _href "/styles.css" ]
            meta [ _name "viewport"
                   _content "width=device-width, initial-scale=1" ]
            meta [ _name "description"
                   _content metaDescription ]
            baseTag [_href "site"]

        ]
        body [] [
            nav [] [
                div [] [
                    a [ _href "/" ] [
                        encodedText "Tao Te Ching"
                    ]
                ]
            ]
            main [ _class "mainContent" ] [ page ]

        ]
    ]

let renderChapter (chapter: Chapter) =
    div [] [
        h1 [] [encodedText (chapter.Id |> string)]
        article [ _class "article" ] [
            rawText chapter.Text
        ]
    ]
    |> renderLayout (sprintf "Chapter %i" chapter.Id) (sprintf "Chapter %i of the Tao Te Ching" chapter.Id)

let renderHome (chapterList: Chapter list) =
    main [] [
        div [ _class "chapterList"] 
            (chapterList
            |> List.map (fun c -> 
                div [ _class "chapterCard"] [
                    a [_href (c.Id|>string)] [encodedText (c.Id|>string)]
                ]
            ))
    ]
    |> renderLayout "Welcome to my blog" "Here is my blog"