package windriver;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import org.openqa.selenium.remote.codec.AbstractHttpCommandCodec;

import com.google.common.collect.ImmutableMap;

import static org.openqa.selenium.remote.DriverCommand.ACTIONS;
import static org.openqa.selenium.remote.DriverCommand.CLEAR_ACTIONS_STATE;
import static org.openqa.selenium.remote.DriverCommand.GET_ACTIVE_ELEMENT;
import static org.openqa.selenium.remote.DriverCommand.GET_CURRENT_WINDOW_HANDLE;
import static org.openqa.selenium.remote.DriverCommand.GET_CURRENT_WINDOW_POSITION;
import static org.openqa.selenium.remote.DriverCommand.GET_CURRENT_WINDOW_SIZE;
import static org.openqa.selenium.remote.DriverCommand.GET_ELEMENT_ATTRIBUTE;
import static org.openqa.selenium.remote.DriverCommand.GET_PAGE_SOURCE;
import static org.openqa.selenium.remote.DriverCommand.GET_WINDOW_HANDLES;
import static org.openqa.selenium.remote.DriverCommand.IS_ELEMENT_DISPLAYED;
import static org.openqa.selenium.remote.DriverCommand.MAXIMIZE_CURRENT_WINDOW;
import static org.openqa.selenium.remote.DriverCommand.MINIMIZE_CURRENT_WINDOW;
import static org.openqa.selenium.remote.DriverCommand.SEND_KEYS_TO_ELEMENT;
import static org.openqa.selenium.remote.DriverCommand.SET_CURRENT_WINDOW_POSITION;
import static org.openqa.selenium.remote.DriverCommand.SET_CURRENT_WINDOW_SIZE;

public class WinDriverCommandCodec extends AbstractHttpCommandCodec {

    public WinDriverCommandCodec() {
        String sessionId = "/session/:sessionId";
        defineCommand(GET_PAGE_SOURCE, get(sessionId + "/source"));
        defineCommand(ACTIONS, post(sessionId + "/actions"));
        defineCommand(CLEAR_ACTIONS_STATE, delete(sessionId + "/actions"));

        String elementId = sessionId + "/element/:id";
        defineCommand(GET_ELEMENT_ATTRIBUTE, get(elementId + "/attribute/:name"));
        defineCommand(IS_ELEMENT_DISPLAYED, get(elementId + "/displayed"));
        
        String window = sessionId + "/window";
        defineCommand(GET_WINDOW_HANDLES, get(window + "/handles"));
        defineCommand(GET_CURRENT_WINDOW_HANDLE, get(window));
        defineCommand(GET_CURRENT_WINDOW_POSITION, get(window + "/rect"));
        defineCommand(SET_CURRENT_WINDOW_POSITION, post(window + "/rect"));
        defineCommand(GET_CURRENT_WINDOW_SIZE, get(window + "/rect"));
        defineCommand(SET_CURRENT_WINDOW_SIZE, post(window + "/rect"));
        defineCommand(MAXIMIZE_CURRENT_WINDOW, post(window + "/maximize"));
        defineCommand(MINIMIZE_CURRENT_WINDOW, post(window + "/minimize"));

        defineCommand(GET_ACTIVE_ELEMENT, get(sessionId + "/element/active"));
    }

    private List<String> stringToUtf8Array(String toConvert) {
        List<String> toReturn = new ArrayList<>();
        int offset = 0;
        while (offset < toConvert.length()) {
        int next = toConvert.codePointAt(offset);
        toReturn.add(new StringBuilder().appendCodePoint(next).toString());
        offset += Character.charCount(next);
        }
        return toReturn;
    }

    @SuppressWarnings("unchecked")
    @Override
    protected Map<String, ?> amendParameters(String name, Map<String, ?> parameters) {
        switch (name) {
            case SEND_KEYS_TO_ELEMENT:
                // When converted from JSON, this is a list, not an array
                Object rawValue = parameters.get("value");
                Stream<CharSequence> source;
                if (rawValue instanceof Collection) {
                //noinspection unchecked
                source = ((Collection<CharSequence>) rawValue).stream();
                } else {
                source = Stream.of((CharSequence[]) rawValue);
                }

                String text = source.collect(Collectors.joining());
                return ImmutableMap.<String, Object>builder()
                    .putAll(
                        parameters.entrySet().stream()
                            .filter(e -> !"text".equals(e.getKey()))
                            .filter(e -> !"value".equals(e.getKey()))
                            .collect(Collectors.toMap(Map.Entry::getKey, Map.Entry::getValue)))
                    .put("text", text)
                    .put("value", stringToUtf8Array(text))
                    .build();
            default:
                break;
        }
        return parameters;
    }
}
