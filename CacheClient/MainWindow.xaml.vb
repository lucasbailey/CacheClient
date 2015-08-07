Imports TCPServerApp.TCPServerApp 'contains the available options for the combo boxes
Imports TCPClientApp.TCPClientApp
Imports System.Web.Script.Serialization

Class MainWindow
    Private Sub Button_INSERT_Click(sender As Object, e As RoutedEventArgs)
        Dim serializer As New JavaScriptSerializer()
        Dim thePayload As TCPServerApp.TCPServerApp.PayloadContainer
        thePayload = BuildThePayload(CommandDrop.SelectedItem.Key, CommandTypeDrop.SelectedItem.Key, _
                                     ConnectionStringDrop.SelectedItem.Key, PayloadDrop.SelectedItem.Key)


        PreviewLabel.Content = serializer.Serialize(thePayload)

        Dim theNewClient As New TCPClientApp.TCPClientApp({PreviewLabel.Content & System.Environment.NewLine})

        ServerResults.Text = theNewClient.ExecuteCommand(PortDrop.SelectedValue.value)
    End Sub

    Private Function BuildThePayload(command As String, Optional commandType As String = "", _
                                     Optional ConnectionString As String = "", Optional Payload As String = "")

        Dim thePayload As New TCPServerApp.TCPServerApp.PayloadContainer

        thePayload.Command = command
        thePayload.CommandType = commandType
        thePayload.ConnectionString = ConnectionString
        thePayload.Payload = Payload

        Return thePayload
    End Function

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        PopulatePortsList()

        CommandTypeDrop.ItemsSource = TCPServerApp.TCPServerApp.PayloadContainer.PayloadCommandTypes.AvailOptions
        CommandDrop.ItemsSource = TCPServerApp.TCPServerApp.PayloadContainer.PayloadCommands.AvailOptions

        Dim PayloadDictionary As New Dictionary(Of String, String) From _
            {
                {
                    "SELECT * FROM [Dashboards].[dbo].[ChartParameter]",
                    "Dashboard SQL Query"
                },
                {
                    "Y:\Projects\Toolbox\Toolbox\Analytics\analytics.json",
                    "Analytics JSON File"
                },
                {
                    "Y:\Projects\Toolbox\Toolbox\Analytics\analytics.xml",
                    "Analytics XML File"
                },
                {
                    "{""127.0.0.1"" : ""127.0.0.1""}",
                    "IP Address To Register"
                }
            }

        Dim AvailStringOptions As New Dictionary(Of String, String) From _
            {
                {
                    "Server=localhost;Database=Dashboards;User Id=sa;Password=szfzpk501!",
                    "localhost / Dashboards / sa"
                },
                {
                    "Y:\Projects\Toolbox\Toolbox\MinifyConfig\Analytics\analytics.json",
                    "JSON Analytics Minify Config File"
                }
            }

        ConnectionStringDrop.ItemsSource = AvailStringOptions
        PayloadDrop.ItemsSource = PayloadDictionary

        If CommandDrop.Items.Count > 0 Then
            CommandDrop.SelectedIndex = 0
        End If

        If CommandTypeDrop.Items.Count > 0 Then
            CommandTypeDrop.SelectedIndex = 0
        End If

        If ConnectionStringDrop.Items.Count > 0 Then
            ConnectionStringDrop.SelectedIndex = 0
        End If

        If PayloadDrop.Items.Count > 0 Then
            PayloadDrop.SelectedIndex = 0
        End If

    End Sub

    Private Sub PopulatePortsList()
        Dim serializer As New JavaScriptSerializer
        Dim thePayload As TCPServerApp.TCPServerApp.PayloadContainer


        thePayload = BuildThePayload(TCPServerApp.TCPServerApp.PayloadContainer.PayloadCommands.LOCAL_PORTS)

        Dim theNewClient As New TCPClientApp.TCPClientApp({serializer.Serialize(thePayload) & System.Environment.NewLine})

        Dim PortsDictionary As Dictionary(Of String, Long) = serializer.Deserialize(Of Dictionary(Of String, Long))(theNewClient.ExecuteCommand())

        PortDrop.ItemsSource = PortsDictionary

        'automatically select the default port
        For Each value In PortDrop.Items
            If value.key = TCPServerApp.TCPServerApp.DEFAULT_PORT.ToString Then
                PortDrop.SelectedItem = value
                Exit For
            End If
        Next
    End Sub
End Class
