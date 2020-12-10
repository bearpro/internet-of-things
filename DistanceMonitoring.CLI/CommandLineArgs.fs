module DistanceMonitoring.CLI.CommandLineArgs
open CommandLine
[<Verb("run-mock", HelpText = "Запускает mock-объект.")>]
type MockOptions = { 
    [<Option(Required=true, HelpText="Адрес сервера")>] Ip: string
    [<Option(Required=false, Default=1883, HelpText="Номер порта сервера")>] Port: int
    [<Option(Required=true, HelpText="Идентификатор клиента")>] ClientId: string
    [<Option(Required=true, HelpText="Конфигурационный файл")>] ConfigPath: string
}
[<Verb("shit", HelpText = "")>]
type ShitOptions = { 
    [<Value(0, MetaName="bar", HelpText="___")>] foo: string
}