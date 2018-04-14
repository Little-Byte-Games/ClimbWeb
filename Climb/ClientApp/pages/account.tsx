import '../css/site.less';
import '../css/pages/account.less';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { BrowserRouter } from 'react-router-dom';
import { Layout } from '../components/account/Layout';

function renderApp() {
    // This code starts up the React app when it runs in a browser. It sets up the routing
    // configuration and injects the app into a DOM element.
    const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;
    ReactDOM.render(
        <AppContainer>
            <BrowserRouter basename={ baseUrl }>
                <div>
                    <Layout/>
                </div>
            </BrowserRouter>
        </AppContainer>,
        document.getElementById('react-app')
    );
}

renderApp();