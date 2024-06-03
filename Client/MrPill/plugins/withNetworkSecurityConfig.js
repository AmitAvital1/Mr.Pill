const path = require('path');
const { withAndroidManifest, withDangerousMod } = require('@expo/config-plugins');
const fs = require('fs');

const NETWORK_SECURITY_CONFIG = `
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <domain-config>
        <!-- Localhost config is NEEDED from react-native for the bundling to work  -->
        <domain-config cleartextTrafficPermitted="true">
            <domain includeSubdomains="true">127.0.0.1</domain>
            <domain includeSubdomains="true">10.0.0.1</domain>
            <domain includeSubdomains="true">localhost</domain>
        </domain-config>

        <domain includeSubdomains="true">my.domain.com</domain>
        <trust-anchors>
            <certificates src="@raw/ca"/>
            <certificates src="system"/>
        </trust-anchors>
    </domain-config>
</network-security-config>
`;

const withNetworkSecurityConfig = (config) => {
  config = withDangerousMod(config, [
    'android',
    async (config) => {
      const filePath = path.join(config.modRequest.platformProjectRoot, 'app/src/main/res/xml/network_security_config.xml');

      await fs.promises.mkdir(path.dirname(filePath), { recursive: true });
      await fs.promises.writeFile(filePath, NETWORK_SECURITY_CONFIG);

      return config;
    },
  ]);

  return withAndroidManifest(config, async (config) => {
    const androidManifest = config.modResults;

    const application = androidManifest.manifest.application;
    if (!application[0]['android:networkSecurityConfig']) {
      application[0]['android:networkSecurityConfig'] = '@xml/network_security_config';
    }

    return config;
  });
};

module.exports = withNetworkSecurityConfig;