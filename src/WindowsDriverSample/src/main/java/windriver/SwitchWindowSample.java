package windriver;

import java.net.URI;

import org.openqa.selenium.By;
import org.openqa.selenium.Dimension;
import org.openqa.selenium.Point;
import org.openqa.selenium.remote.RemoteWebDriver;

import io.kanthis.WinDriver;
import io.kanthis.WinDriverOptions;
import io.kanthis.by.AutomationIdBy;

public class SwitchWindowSample {
    public static void main(String[] args) throws Exception {
        WinDriverOptions cap = new WinDriverOptions().setAumid("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        RemoteWebDriver driver = new WinDriver(URI.create("http://localhost:5000").toURL(), cap);

        try {
            System.out.println(driver.getWindowHandle());
            System.out.println(driver.getTitle());
            System.out.println(driver.manage().window().getPosition());
            System.out.println(driver.manage().window().getSize());

            driver.findElement(new AutomationIdBy("TogglePaneButton")).click();
            Thread.sleep(500);
            driver.findElement(new AutomationIdBy("SettingsItem")).click();
            Thread.sleep(500);
            driver.findElement(new AutomationIdBy("FeedbackButton")).click();
            Thread.sleep(500);

            for (String wnd: driver.getWindowHandles()) {
                driver.switchTo().window(wnd);
                if (driver.getTitle().equals("Feedback Hub")) {
                    break;
                }
            };

            driver.manage().window().setPosition(new Point(0, 0));
            Thread.sleep(1000);
            driver.manage().window().setSize(new Dimension(100, 100));
            Thread.sleep(1000);
            Thread.sleep(1000);
            driver.manage().window().maximize();
            Thread.sleep(1000);
            driver.manage().window().minimize();
            Thread.sleep(1000);

            WinDriverOptions capRoot = new WinDriverOptions().setMaxTreeDepth(2);
            RemoteWebDriver driverRoot = new WinDriver(URI.create("http://localhost:5000").toURL(), capRoot);

            try {
                String handle = driverRoot.findElement(By.xpath("//Pane[@Name=\"Taskbar\"]")).getAttribute("NativeWindowHandle");
                WinDriverOptions capInject = new WinDriverOptions().setNativeWindowHandle(Integer.parseInt(handle));
                RemoteWebDriver driverInject = new WinDriver(URI.create("http://localhost:5000").toURL(), capInject);

                try {
                    driverInject.findElement(By.xpath("//Button[contains(@Name, \"Feedback Hub\")]")).click();
                } catch (Exception ex) {
                    ex.printStackTrace();
                } finally {
                    driverInject.quit();
                }
                Thread.sleep(5000);
            } catch (Exception ex) {
                ex.printStackTrace();
            } finally {
                driverRoot.quit();
            }

        } catch (Exception ex) {
            ex.printStackTrace();
        } finally {
            driver.quit();
        }
    }
}
