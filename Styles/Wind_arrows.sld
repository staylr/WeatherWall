<StyledLayerDescriptor version="1.0.0"
  xmlns="http://www.opengis.net/sld" xmlns:gml="http://www.opengis.net/gml"
  xmlns:ogc="http://www.opengis.net/ogc"
xmlns:xlink="http://www.w3.org/1999/xlink"
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
  xsi:schemaLocation="http://www.opengis.net/sld ./StyledLayerDescriptor.xsd">
  <NamedLayer>
    <Name>contour_lines</Name>
    <UserStyle>
      <FeatureTypeStyle>
        <Transformation>
          <ogc:Function name="gs:RasterAsPointCollection">
            <ogc:Function name="parameter">
              <ogc:Literal>data</ogc:Literal>
            </ogc:Function>
          </ogc:Function>
        </Transformation>
        <Rule>
          <TextSymbolizer>
            <Label><![CDATA[ ]]></Label> <!-- fake label -->
            <Graphic>
              <Mark>
                <WellKnownName>shape://carrow</WellKnownName>
                <Fill>
                  <CssParameter name="fill">#000000</CssParameter>
                </Fill>
              </Mark>
              <Size>
                <ogc:Literal>200</ogc:Literal>
              </Size>
              <Rotation>
				<ogc:PropertyName>Band1</ogc:PropertyName>
              </Rotation>
            </Graphic>
          </TextSymbolizer>
        </Rule>
      </FeatureTypeStyle>
    </UserStyle>
  </NamedLayer>
</StyledLayerDescriptor>