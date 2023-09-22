export const hair_shader = {
  label: "hair shader",
  code: `
        struct VertexOut{
          @builtin(position) position : vec4f,
          @location(0) color : vec4f,
        }

        @group(0) @binding(0) var<uniform> transform: mat4x4<f32>;
        @group(0) @binding(1) var<uniform> screen: vec2f;        

        @vertex
        fn vertexMain(@location(0) position: vec2f, @location(1) color: vec4f) -> VertexOut {
          // dummy so auto layout sees the variable 
          let dummy = screen;
          return VertexOut(transform*vec4f(position, 0, 1), color);                  
        }         

        @fragment
        fn fragmentMain(vertexOut: VertexOut) -> @location(0) vec4f {
          return vertexOut.color;
        }
      `
};

export const fastline_shader = {
  label: "line shader",
  code: `
        @group(0) @binding(0) var<uniform> transform: mat4x4<f32>;
        @group(0) @binding(1) var<uniform> screen: vec2f;

        struct VertexOut{
          @builtin(position) position : vec4f,
          @location(0) color : vec4f,
        }

        @vertex
        fn vertexMain(@location(0) A: vec2f, @location(1) B: vec2f, @location(2) C: vec2f,@location(3) colorAndDir: vec4f)
          -> VertexOut {                             

          // thickness in pixel
          let thickness = 1.0;               
          let uscreen = vec2f(1, screen.y/screen.x);       
          // half of linewidth in normal space (= -1 to 1)
          let uwidth2 = thickness*1.0/screen.x;

          var a = (transform*vec4f(A, 0, 1)).xy;
          let b = (transform*vec4f(B, 0, 1)).xy;
          var c = (transform*vec4f(C, 0, 1)).xy;
          if(length(b-a)==0)
            {a = b + normalize(b-c);}
          if(length(b-c)==0)
            {c = b + normalize(b-a);}
          let ab = normalize(normalize((b-a))*uscreen);
          let bc = normalize(normalize((c-b))*uscreen);
          let tangent = normalize(ab+bc);
          let miter = vec2f(-tangent.y,tangent.x);
          let normalA = vec2f(-ab.y,ab.x);
          let miterLength = 1 / dot(miter, normalA);
          return VertexOut(vec4f(b+sign(colorAndDir.w)*uwidth2*miter*miterLength/uscreen , 0, 1), colorAndDir);          
        }         

        @fragment
        fn fragmentMain(vertexOut: VertexOut) -> @location(0) vec4f {
          return vec4f(vertexOut.color.xyz,1);
        }
      `
};

export const aa_fastline_shader = {
  label: "line shader with aa",
  code: `
        @group(0) @binding(0) var<uniform> transform: mat4x4<f32>;
        @group(0) @binding(1) var<uniform> screen: vec2f;

        struct VertexOut{
          @builtin(position) position : vec4f,
          @location(0) color : vec4f,
          // distance of the vertex from the line in screen space 
          @location(1)@interpolate(linear) screenDist : vec2f,
        }

        @vertex
        fn vertexMain(@location(0) A: vec2f, @location(1) B: vec2f, @location(2) C: vec2f,@location(3) colorAndDir: vec4f)
          -> VertexOut {                             

          // thickness in pixel but wider as the purpose is only to capture enough pixel so the fragment shader us invoked on them
          let thickness = 12.0;
          let radius = 6.0;
          let uscreen = vec2f(1, screen.y/screen.x);       
          let uwidth = thickness*1.0/screen.x;

          var a = (transform*vec4f(A, 0, 1)).xy;
          let b = (transform*vec4f(B, 0, 1)).xy;
          var c = (transform*vec4f(C, 0, 1)).xy;
          if(length(b-a)==0)
            {a = b + normalize(b-c);}
          if(length(b-c)==0)
            {c = b + normalize(b-a);}
          let ab = normalize(normalize((b-a))*uscreen);
          let bc = normalize(normalize((c-b))*uscreen);
          let tangent = normalize(ab+bc);
          let miter = vec2f(-tangent.y,tangent.x);
          let normalA = vec2f(-ab.y,ab.x);
          let miterLength = 1 / dot(miter, normalA);

          let screenDist = select(vec2f(-radius,0),vec2f(radius,0),colorAndDir.w<0);
          return VertexOut(vec4f(b+sign(colorAndDir.w)*uwidth*miter*miterLength/uscreen , 0, 1), colorAndDir, screenDist);          
        }         

        @fragment
        fn fragmentMain(in: VertexOut) -> @location(0) vec4f {
          let radius = 0.5;
          let s = 0.5;
          var x = abs(in.screenDist.x);
          var a = 1-smoothstep(radius-s, radius+s, x);                 
          return vec4f(in.color.xyz, a);
        }
      `
};