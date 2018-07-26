namespace Fable.Import
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS

module bodyParser =
    type [<AllowNullLiteral>] Options =
        abstract inflate: bool option with get, set
        abstract limit: U2<float, string> option with get, set
        abstract ``type``: U3<string, ResizeArray<string>, Func<express.Request, obj>> option with get, set
        abstract verify: req: express.Request * res: express.Response * buf: Buffer * encoding: string -> unit

    and [<AllowNullLiteral>] OptionsJson =
        inherit Options
        abstract strict: bool option with get, set
        abstract reviver: key: string * value: obj -> obj

    and [<AllowNullLiteral>] OptionsText =
        inherit Options
        abstract defaultCharset: string option with get, set

    and [<AllowNullLiteral>] OptionsUrlencoded =
        inherit Options
        abstract extended: bool option with get, set
        abstract parameterLimit: float option with get, set

    type [<Import("*","body-parser")>] Globals =
        static member json(?options: OptionsJson): express.RequestHandler = jsNative
        static member raw(?options: Options): express.RequestHandler = jsNative
        static member text(?options: OptionsText): express.RequestHandler = jsNative
        static member urlencoded(?options: OptionsUrlencoded): express.RequestHandler = jsNative
