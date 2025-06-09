let dotnetHelper = null;

function setDotnetHelper(helper) {
    dotnetHelper = helper;
}

async function onTelegramAuth(user) {
    console.log("Received from Telegram:", user);

    if (dotnetHelper) {
        // const jsonString = JSON.stringify(user);
        await dotnetHelper.invokeMethodAsync('ReceiveTelegramAuthResult', user);
    } else {
        console.error("DotNet helper not set");
    }
}