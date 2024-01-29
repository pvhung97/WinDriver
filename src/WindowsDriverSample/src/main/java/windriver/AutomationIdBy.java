package windriver;

import java.util.List;

import org.openqa.selenium.By;
import org.openqa.selenium.SearchContext;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.By.Remotable;

public class AutomationIdBy extends By implements Remotable  {
    private final Parameters params;

    public AutomationIdBy(String automationId) {
        this.params = new Parameters("automation id", automationId);
    }

    @Override
    public WebElement findElement(SearchContext context) {
        return context.findElement(this);
    }

    @Override
    public List<WebElement> findElements(SearchContext context) {
        return context.findElements(this);
    }

    @Override
    public Parameters getRemoteParameters() {
        return params;
    }
    
}
