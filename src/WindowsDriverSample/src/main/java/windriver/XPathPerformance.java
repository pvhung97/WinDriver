package windriver;

import java.net.URI;

import org.openqa.selenium.By;
import org.openqa.selenium.remote.RemoteWebDriver;

import io.kanthis.WinDriver;
import io.kanthis.WinDriverOptions;

public class XPathPerformance {
    public static void main(String[] args) throws Exception {
        WinDriverOptions cap = new WinDriverOptions().setAumid("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        RemoteWebDriver driver = new WinDriver(URI.create("http://localhost:5000").toURL(), cap);

        try {
            long start = System.currentTimeMillis();
            driver.findElement(By.xpath("/Window[1]/Window[2]/Custom[1]/Group[1]/Group[2]/Button[6]"));
            System.out.println("Take: " + (System.currentTimeMillis() - start));

            start = System.currentTimeMillis();
            driver.findElement(By.xpath("/Window/Window[@Name='Calculator']/Custom/Group/Group/Button[@Name='Five']"));
            System.out.println("Take: " + (System.currentTimeMillis() - start));

            start = System.currentTimeMillis();
            driver.findElement(By.xpath("/Window/Window[2]/Custom//Button[@Name='Five']"));
            System.out.println("Take: " + (System.currentTimeMillis() - start));

            start = System.currentTimeMillis();
            driver.findElement(By.xpath("//Button[@Name='Five']"));
            System.out.println("Take: " + (System.currentTimeMillis() - start));
        } catch (Exception ex) {
            ex.printStackTrace();
        } finally {
            driver.quit();
        }
    }
}
