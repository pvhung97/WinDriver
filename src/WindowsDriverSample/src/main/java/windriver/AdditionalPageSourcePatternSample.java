package windriver;

import java.net.URI;
import java.util.List;

import org.openqa.selenium.By;
import org.openqa.selenium.remote.RemoteWebDriver;

import io.kanthis.WinDriver;
import io.kanthis.WinDriverOptions;
import io.kanthis.dto.PageSourceAdditionalPattern;

public class AdditionalPageSourcePatternSample {

    public static void main(String[] args) throws Exception {
        WinDriverOptions cap = new WinDriverOptions().setAumid("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App").setAdditionalPageSourcePattern(List.of(PageSourceAdditionalPattern.LEGACY_I_ACCESSIBLE));
        RemoteWebDriver driver = new WinDriver(URI.create("http://localhost:5000").toURL(), cap);

        try {
            driver.findElement(By.xpath("//*[@LegacyIAccessible_Name='Five']")).click();
            System.out.println(driver.getPageSource());
        } finally {
            driver.quit();
        }
    }
    
}
