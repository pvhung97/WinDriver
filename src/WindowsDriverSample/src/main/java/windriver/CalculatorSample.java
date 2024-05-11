package windriver;

import java.net.URI;

import org.openqa.selenium.By;
import org.openqa.selenium.OutputType;
import org.openqa.selenium.Platform;
import org.openqa.selenium.Rectangle;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.remote.RemoteWebDriver;

public class CalculatorSample {
    public static void main(String[] args) throws Exception {
        DesiredCapabilities cap = new DesiredCapabilities();
        cap.setPlatform(Platform.WINDOWS);
        cap.setCapability("windriver:aumid", "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        cap.setCapability("windriver:automationName", "uia3");
        RemoteWebDriver driver = new WinDriver(URI.create("http://localhost:5085").toURL(), cap);

        try {
            System.out.println(driver.findElements(By.xpath("//Button")).size());
            System.out.println(driver.findElement(By.xpath("/Window")).findElements(By.tagName("Text")).size());

            System.out.println(driver.findElement(new AutomationIdBy("num5Button")).getAttribute("Name"));
            Rectangle rect = driver.findElement(new NameBy("Five")).getRect();
            System.out.println(rect.getPoint());
            System.out.println(rect.getDimension());
            System.out.println(driver.findElement(new AutomationIdBy("num5Button")).isDisplayed());
            System.out.println(driver.findElement(new AutomationIdBy("num5Button")).isEnabled());
            System.out.println(driver.findElement(new AutomationIdBy("num5Button")).isSelected());
            driver.findElement(new AutomationIdBy("num5Button")).click();
            Thread.sleep(500);
            System.out.println(driver.findElement(By.xpath("//Button[@Name=\"Five\"]")).findElement(By.xpath("//Text")).getText());
            System.out.println(driver.switchTo().activeElement().getTagName());
            driver.findElement(new AutomationIdBy("TogglePaneButton")).click();
            Thread.sleep(500);
            driver.findElement(new AutomationIdBy("Angle")).click();
            Thread.sleep(500);
            driver.findElement(new AutomationIdBy("TogglePaneButton")).click();
            Thread.sleep(500);
            driver.findElement(new AutomationIdBy("SettingsItem")).click();
            Thread.sleep(500);
            driver.findElement(new AutomationIdBy("FeedbackButton")).click();
            Thread.sleep(5000);

            for (String wnd: driver.getWindowHandles()) {
                driver.switchTo().window(wnd);
                if (driver.getTitle().equals("Feedback Hub")) {
                    break;
                }
            };

            driver.getPageSource();
            driver.getScreenshotAs(OutputType.BASE64);
            driver.findElement(new AutomationIdBy("FeedbackTitleTextBox")).getScreenshotAs(OutputType.BASE64);
            driver.findElement(new AutomationIdBy("FeedbackTitleTextBox")).sendKeys("abc123能力");
            Thread.sleep(1000);
            driver.findElement(new AutomationIdBy("FeedbackTitleTextBox")).clear();
            Thread.sleep(1000);
        } catch (Exception ex) {
            ex.printStackTrace();
        } finally {
            driver.quit();
        }

    }
}