function add-probing ($config) {
    $xml = [xml](Get-Content $config)
    $xml.configuration.runtime.InnerXml = '<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1"><probing privatePath="bin;"/></assemblyBinding>' + $xml.configuration.runtime.InnerXml
    $xml.Save((Resolve-Path $config))
}

add-probing "..\output\Eu.EDelivery.AS4.ServiceHandler.ConsoleHost.dll.config"
add-probing "..\output\Eu.EDelivery.AS4.WindowsService.dll.config"
add-probing "..\output\Eu.EDelivery.AS4.Fe.dll.config"