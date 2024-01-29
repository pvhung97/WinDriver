package windriver;

import java.net.URI;

import org.openqa.selenium.By;
import org.openqa.selenium.Keys;
import org.openqa.selenium.Platform;
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.interactions.PointerInput;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.remote.RemoteWebDriver;

public class ActionsSample {
    public static void main(String[] args) throws Exception {
        DesiredCapabilities cap = new DesiredCapabilities();
        cap.setPlatform(Platform.WINDOWS);
        cap.setCapability("windriver:aumid", "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        cap.setCapability("windriver:automationName", "uia3");
        RemoteWebDriver driver = new WinDriver(URI.create("http://localhost:5058").toURL(), cap);

        try {
            new Actions(driver).sendKeys("5").build().perform();
            Thread.sleep(1000);
            new Actions(driver).setActivePointer(PointerInput.Kind.TOUCH, "default touch").moveToElement(driver.findElement(By.xpath("//Button[@Name=\"Five\"]"))).click().build().perform();
            Thread.sleep(1000);
            int width = driver.manage().window().getSize().getWidth();
            new Actions(driver).setActivePointer(PointerInput.Kind.PEN, "default pen").moveToLocation(width / 2, 10).clickAndHold().pause(50).moveToLocation(width / 2 + width / 2 - 10, 10).build().perform();
            Thread.sleep(1000);
            new Actions(driver).moveToElement(driver.findElement(new AutomationIdBy("TogglePaneButton"))).click().pause(500).build().perform();
            new Actions(driver).moveToElement(driver.findElement(new AutomationIdBy("PaneRoot"))).pause(500).scrollByAmount(0, -12000).build().perform();
            driver.resetInputState();
            new Actions(driver).keyDown(Keys.ALT).keyDown(Keys.F4).keyUp(Keys.F4).keyUp(Keys.ALT).build().perform();
            Thread.sleep(5000);
        } catch (Exception ex) {
            ex.printStackTrace();
        } finally {
            driver.quit();
        }
    }
}
