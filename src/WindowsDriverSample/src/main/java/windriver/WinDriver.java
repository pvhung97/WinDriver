package windriver;

import static org.openqa.selenium.remote.DriverCommand.NEW_SESSION;

import java.io.IOException;
import java.lang.reflect.Field;
import java.net.URL;

import org.openqa.selenium.Capabilities;
import org.openqa.selenium.remote.Command;
import org.openqa.selenium.remote.CommandExecutor;
import org.openqa.selenium.remote.HttpCommandExecutor;
import org.openqa.selenium.remote.RemoteWebDriver;
import org.openqa.selenium.remote.Response;
import org.openqa.selenium.remote.http.ClientConfig;

public class WinDriver extends RemoteWebDriver {
    
    public WinDriver(URL remoteAddress, Capabilities capabilities) {
        super(createCommandExecutor(remoteAddress), capabilities);
    }

    private static CommandExecutor createCommandExecutor(URL remoteAddress) {
        ClientConfig config = ClientConfig.defaultConfig().baseUrl(remoteAddress);
        return new HttpCommandExecutor(config) {

            @Override
            public Response execute(Command command) throws IOException {
                Response response = super.execute(command);
                if (NEW_SESSION.equals(command.getName())) {
                    try {
                        Field commandCodec = null;
                        commandCodec = this.getClass().getSuperclass().getDeclaredField("commandCodec");
                        commandCodec.setAccessible(true);
                        commandCodec.set(this, new WinDriverCommandCodec());
                    } catch (Exception ex) {
                        ex.printStackTrace();
                    }
                }
                return response;
            }

        };
    }
    
}
