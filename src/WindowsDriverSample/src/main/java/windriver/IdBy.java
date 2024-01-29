package windriver;

import java.util.List;

import org.openqa.selenium.By;
import org.openqa.selenium.By.Remotable;
import org.openqa.selenium.SearchContext;
import org.openqa.selenium.WebElement;

public class IdBy extends By implements Remotable  {
    private final Parameters params;

    public IdBy(String id) {
        this.params = new Parameters("id", id);
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

